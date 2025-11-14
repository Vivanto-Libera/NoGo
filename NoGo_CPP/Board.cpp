#include "Board.h"

GoString GoString::merge(GoString goString)
{
	std::unordered_set<Point> newStones;
	newStones.insert(stones.begin(), stones.end());
	newStones.insert(goString.stones.begin(), goString.stones.end());
	std::unordered_set<Point> newLiberties;
	newLiberties.insert(liberties.begin(), liberties.end());
	newLiberties.insert(goString.liberties.begin(), goString.liberties.end());
	newLiberties.erase(newStones.begin(), newStones.end());
	return GoString(color, newStones, newLiberties);
}

const int Board::rowDirection[4]{ 1, -1, 0, 0 };
const int Board::colDirection[4]{ 0, 0, 1, -1 };

std::vector<int> Board::legalMoves()
{
	std::vector<int> moves;
	for (int i = 0; i < 9;i ++)
	{
		for (int j = 0; j < 9; j++)
		{
			if (board[i][j] == EMPTY)
			{
				moves.emplace_back(indexToNum(i, j));
			}
		}
	}
	return moves;
}
void Board::applyMove(int move)
{
	int row = move / 10;
	int col = move % 10;
	board[row][col] = turn;
	std::unordered_set<GoString> sameColor;
	std::unordered_set<GoString> oppositeColor;
	std::unordered_set<Point> liberties;
	for (int i = 0; i < 4; i++)
	{
		int newRow = row + rowDirection[i];
		int newCol = col + colDirection[i];
		if (newRow < 0 || newRow > 8 || newCol < 0 || newCol > 8)
		{
			break;
		}
		if (board[newRow][newCol] == EMPTY)
		{
			liberties.insert(Point(newRow, newCol));
			break;
		}
		if (board[newRow][newCol] == turn)
		{
			GoString same = findString(Point(newRow, newCol));
			sameColor.insert(same);
		}
		else
		{
			oppositeColor.insert(findString(Point(newRow, newCol)));
		}
	}
	GoString newString(turn, std::unordered_set<Point>{Point(row, col)}, liberties);
	for (const auto& same : sameColor)
	{
		newString = newString.merge(same);
		strings.erase(same);
	}
	for (auto& opp : oppositeColor)
	{
		strings.erase(opp);
		GoString newOpp = opp;
		newOpp.removeLiberties(Point(row, col));
		strings.insert(newOpp);
	}
	strings.insert(newString);
	if (turn == BLACK)
	{
		turn = WHITE;
	}
	else
	{
		turn = BLACK;
	}
}
Color Board::isTerminal()
{
	for (const auto& string : strings)
	{
		if (string.numLiberties() == 0)
		{
			return turn;
		}
	}
	return EMPTY;
}
py::tuple Board::netwrokInput()
{
	std::array <std::array<int, 9>, 9> playerStone;
	std::array <std::array<int, 9>, 9> oppsiteStone;
	std::array <std::array<int, 9>, 9> playerLiberties;
	std::array <std::array<int, 9>, 9> oppsiteLiberties;
	for (int i = 0; i < 9; i++)
	{
		for (int j = 0; j < 9; j++)
		{
			playerLiberties[i][j] = 0;
			oppsiteLiberties[i][j] = 0;
			if(board[i][j] == turn)
			{
				playerStone[i][j] = 1;
				oppsiteStone[i][j] = 0;
			}
			else if(board[i][j] == EMPTY)
			{
				playerStone[i][j] = 0;
				oppsiteStone[i][j] = 0;
			}
			else
			{
				playerStone[i][j] = 0;
				oppsiteStone[i][j] = 1;
			}
		}
	}
	for (const auto& string : strings)
	{
		if(string.color == turn)
		{
			for (const auto& point : string.liberties)
			{
				playerLiberties[point.row][point.col] = 1;
			}
		}
		else
		{
			for (const auto& point : string.liberties)
			{
				oppsiteLiberties[point.row][point.col] = 1;
			}
		}
	}
	return py::make_tuple(playerStone, oppsiteStone, playerLiberties, oppsiteLiberties);
}

Board::Board()
{
	for (auto& b : board)
	{
		b.fill(EMPTY);
	}
	turn = BLACK;
}
Board::Board(const Board& aBoard)
{
	board = aBoard.board;
	strings = aBoard.strings;
	turn = aBoard.turn;
}
