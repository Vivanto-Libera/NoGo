import numpy as np
import os
import mcts
import torch
import math
from NoGo import Board
from NoGo import Color
from tqdm import tqdm
import numpy as np
from nogoModel import NoGoModel
import time
from concurrent.futures import ProcessPoolExecutor
import random
import sys
from pathlib import Path
from agent import Agent
from dataset import NoGoDataset
import torch.optim as optim
from torch.utils.data import Dataset, DataLoader
import torch.nn as nn
def genarate_more_pos(pos, probs, values):
    allPos = np.empty(0)
    allProbs = np.empty(0)
    allValues = np.empty(0)

    for k in range(0, 4):
        rotPos = np.rot90(pos, k, axes=(2, 3))
        rotProbs = np.rot90(probs, k, axes=(1, 2))
        if k == 0:
            allPos = rotPos
            allProbs = rotProbs.reshape(-1, 81)
            allValues = values
        else:
            allPos = np.concatenate([rotPos, allPos], axis=0)
            allProbs = np.concatenate([rotProbs.reshape(-1, 81), allProbs], axis=0)
            allValues = np.concatenate([values, allValues], axis=0)
        allPos = np.concatenate([np.flip(rotPos, axis=2), allPos], axis=0)
        allProbs = np.concatenate([np.flip(rotProbs, axis=1).reshape(-1, 81), allProbs], axis=0)
        allValues = np.concatenate([values, allValues], axis=0)
    return (allPos, allProbs, allValues)

def new_vs_old(old, new):
    board = Board()
    s = -1
    while(board.isTerminal() == Color.EMPTY):
        s+=1
        if(board.turn == Color.WHITE):
            board.applyMove(old.selectMove(board, s))
            continue
        else:
            board.applyMove(new.selectMove(board, s))
            continue
    winner = board.isTerminal()
    return winner

def old_vs_new(old, new):
    board = Board()
    s = -1
    while(board.isTerminal() == Color.EMPTY):
        s+=1
        if(board.turn == Color.BLACK):
            board.applyMove(old.selectMove(board, s))
            continue
        else:
            board.applyMove(new.selectMove(board, s))
            continue
    winner = board.isTerminal()
    return winner

def test(_):
    oldModel = NoGoModel()
    newModel = NoGoModel()
    device= torch.device("cuda" if torch.cuda.is_available() else "cpu")
    oldModel.load_state_dict(torch.load(f"model_it{i}.pt",map_location=device))
    oldModel = oldModel.to(device)
    newModel.load_state_dict(torch.load(f"model_it{best}.pt",map_location=device))
    newModel = newModel.to(device)
    newAgent = Agent(newModel)
    oldAgent = Agent(oldModel)
    for  __ in tqdm(range(0,6)):
        winner = new_vs_old(oldAgent, newAgent)
        with open("count.txt", "a", encoding="utf-8") as f:
            if(winner == Color.WHITE):
                f.write("old\n")
            if(winner == Color.BLACK):
                f.write("new\n")
        winner = old_vs_new(oldAgent, newAgent)
        with open("count.txt", "a", encoding="utf-8") as f:
            if(winner == Color.WHITE):
                f.write("new\n")
            if(winner == Color.BLACK):
                f.write("old\n")

def count():
    old = 0
    new = 0
    with open("count.txt", 'r') as f:
        for line in f:
            if "new\n" == str(line):
                new += 1
            else:
                old += 1
    total = old + new
    return old / total

def playGame(model):
    positionsData = []
    probsData = []
    valuesData = []
    g = Board()
    s = 0
    while g.isTerminal() == Color.EMPTY:
        positionsData.append(g.neuralworkInput())
        rootEdge = mcts.Edge(None, None)
        rootEdge.N = 1
        rootNode = mcts.Node(g, rootEdge)
        mctsSearcher = mcts.MCTS(model, 128, s)
        moveProbs = mctsSearcher.search(rootNode)
        outputVec = np.zeros(81)
        for (move, prob) in moveProbs:
            outputVec[move] = prob
        outputVec = outputVec / np.sum(outputVec)
        rand_idx = np.random.multinomial(1, outputVec)
        nextMove = np.where(rand_idx==1)[0][0]
        if(g.turn == Color.WHITE):
            valuesData.append([1])
        else:
            valuesData.append([-1])
        probsData.append(outputVec.reshape(9, 9))
        g.applyMove(nextMove)
        s += 1
    else:
        winner = g.isTerminal()
        for j in range(0, len(valuesData)):
            if(winner == Color.BLACK):
                valuesData[j][0] = valuesData[j][0] * -1.0
            else:
                valuesData[j][0] = valuesData[j][0] * 1.0
    return (positionsData, probsData, valuesData)

def saveExp(thread):
    allPos = np.empty(0)
    allProbs = np.empty(0)
    allValues = np.empty(0)
    model = NoGoModel()
    model.load_state_dict(torch.load(f"model_it{best}.pt"))
    for _ in tqdm(range(0,25)):
        pos, probs, values = playGame(model)
        pos, probs, values = genarate_more_pos(pos, probs, values)
        if allPos.size == 0:
            allPos = pos
            allProbs = probs
            allValues = values
        else:
            allPos = np.concatenate([pos, allPos], axis=0)
            allProbs = np.concatenate([probs, allProbs], axis=0)
            allValues = np.concatenate([values, allValues], axis=0)
    allPos = np.array(allPos)
    allProbs = np.array(allProbs)
    allValues = np.array(allValues)
    filePath = f'exp_tr{thread}_it{i}.npz'
    np.savez_compressed(
        filePath,
        pos=allPos,
        probs=allProbs,
        values=allValues
    )


i = 0
threads = 16
first = True
for k in range(0, 100000000000):
    if not os.path.exists("model_it"+str(k)+".pt"):
        i = k - 1
        break
best = 0
with open("model_best.txt", 'r') as f:
    best = int(f.read().rstrip('\n'))
while True:
    if os.path.exists(f'model_it{i}.pt'):
        time.sleep(5)
        if not first:
            if Path("count.txt").exists():
                Path("count.txt").unlink()
            with ProcessPoolExecutor(max_workers=threads) as executor:
                executor.map(test, range(threads))
            ct = count()
            with open("log.txt", "a", encoding="utf-8") as f:
                f.write(f"It{i}: win rate = {ct:.2f}\n")
            if ct >= 0.52:
                with open("log.txt", "a", encoding="utf-8") as f:
                    f.write(f"It{i} is new model_best.\n")
                with open("model_best.txt", "w", encoding="utf-8") as f:
                    f.write(f"{i}")
                with open("model_list.txt", "a", encoding="utf-8") as f:
                    f.write(f"{i}\n")
                best = i
        with ProcessPoolExecutor(max_workers=threads) as executor:
            executor.map(saveExp, range(threads))
        i += 1
        first = False
    else:
        time.sleep(30)
    
