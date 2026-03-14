import torch
from NoGo import Board
from NoGo import Color
from agent import Agent
from tqdm import tqdm
from nogoModel import NoGoModel
from print_board import print_board

ROWS = {'9':0,'8':1,'7':2,'6':3,'5':4,'4':5,'3':6,'2':7,'1':8}
COLS = {'a':0,'b':1,'c':2,'d':3,'e':4,'f':5,'g':6,'h':7,'i':8}
model = NoGoModel()
model.load_state_dict(torch.load("new_model.pt", map_location="cpu"))
newAgent = Agent(model)
color = input('Do you want play black or white, enter b or w to select:')
if color == 'b':
     botColor = Color.WHITE
else:
     botColor = Color.BLACK
board = Board()
while(board.isTerminal() == Color.EMPTY):
        print_board(board)
        if(board.turn == botColor):
            move = newAgent.selectMove(board)
            board.applyMove(move)
            continue
        else:
            moves = board.legalMoves()
            while True:
                move = input('Your Move:')
                if len(move) != 2:
                     continue
                if move[0] not in COLS.keys() or move[1] not in ROWS.keys():
                     continue
                col1 = COLS[move[0]]
                row1 = ROWS[move[1]]
                move = row1 * 9 + col1
                if move in moves:
                     board.applyMove(move)
                     break
            continue
            
winner = board.isTerminal()
print_board(board)
print(winner)
