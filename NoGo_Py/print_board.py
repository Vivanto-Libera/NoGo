from NoGo import Board
from NoGo import Color
def print_board(aBoard):
    board = aBoard.board
    rowIndex = '987654321'
    colIndex = 'a  b  c  d  e  f  g  h  i'
    for row in range(0, 9):
        print()
        print(rowIndex[row], end='  ')
        for col in range(0, 9):
            if board[row][col] == Color.EMPTY:
                print('.', end='  ')
            elif board[row][col] == Color.BLACK:
                print('b', end='  ')
            elif board[row][col] == Color.WHITE:
                print('w', end='  ')
    print(f"\n   {colIndex}")