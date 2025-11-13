#include "Board.h"

GoString GoString::merge(GoString goString)
{
	std::set<Point> newStones;
	newStones.insert(stones.begin(), stones.end());
	newStones.insert(goString.stones.begin(), goString.stones.end());
	std::set<Point> newLiberties;
	newLiberties.insert(liberties.begin(), liberties.end());
	newLiberties.insert(goString.liberties.begin(), goString.liberties.end());
	newLiberties.erase(newStones.begin(), newStones.end());
	return GoString(color, newStones, newLiberties);
}


Board::Board()
{
	for (auto& b : board)
	{
		b.fill(Color::EMPTY);
	}
	for (auto& s : pointToString)
	{
		s.fill(nullptr);
	}
	turn = Color::BLACK;
}
Board::Board(const Board& aBoard)
{
	board = aBoard.board;
	strings = aBoard.strings;
	turn = aBoard.turn;
	pointToString = aBoard.pointToString;
}
