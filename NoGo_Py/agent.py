import mcts
from NoGo import Board
import numpy as np

class Agent():

    def __init__(self, model):
        self.model = model

    def selectMove(self, board):
        rootEdge = mcts.Edge(None, None)
        rootEdge.N = 1
        rootNode = mcts.Node(board, rootEdge)
        mctsSearcher = mcts.MCTS(self.model, 10)
        moveProbs = mctsSearcher.search(rootNode)
        outputVec = np.zeros(81)
        for (move, prob) in moveProbs:
            outputVec[move] = prob
        rand_idx = np.random.multinomial(1, outputVec)
        idx = np.where(rand_idx==1)[0][0]
        return idx
