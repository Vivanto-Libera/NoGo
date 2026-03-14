import torch
from NoGo import Board
from NoGo import Color
from agent import Agent
from tqdm import tqdm
from nogoModel import NoGoModel
from print_board import print_board

def bot_vs_bot(board):
    i = 0
    while(board.isTerminal() == Color.EMPTY):
        move = modelAgent.selectMove(board, i)
        print(move)
        board.applyMove(move)
        print_board(board)
        i += 1
    winner = board.isTerminal()
    return winner

num = input("Which Model:")
model = NoGoModel()
device= torch.device("cuda" if torch.cuda.is_available() else "cpu")
model.load_state_dict(torch.load(f"model_it{num}.pt",map_location=device))
model = model.to(device)

modelAgent = Agent(model)
g = Board()
print_board(g)
print(bot_vs_bot(g))
