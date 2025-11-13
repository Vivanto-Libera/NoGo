#ifndef BOARD_H
#define BOARD_H
#include<iostream>
#include<pybind11/pybind11.h>
#include <set>
#include <array>
#include <vector>

namespace py = pybind11;

enum Color
{
	EMPTY,
	WHITE,
	BLACK,
};
struct Point
{
	int row;
	int col;
	Point(int x, int y)
	{
		row = x;
		col = y;
	}
};

class GoString
{
public:
	enum Color color;
	std::set<Point> stones;
	std::set<Point> liberties;

	void removeLiberties(const Point& point)
	{
		liberties.erase(point);
	}
	void addLiberties(const Point& point)
	{
		liberties.insert(point);
	}
	int numLiberties()
	{
		return liberties.size();
	}
	GoString merge(GoString goString);

	GoString::GoString(enum Color color, std::set<Point>& stones, std::set<Point>& liberties) :color(color), stones(stones), liberties(liberties) {}
	~GoString(){}
};

class Board
{
public:
	std::array<std::array<enum Color, 9>, 9> board;
	enum Color turn;
	std::array<std::array<GoString*, 9>, 9> pointToString;
	std::vector<GoString> strings;

	Board();
	Board(const Board& aBoard);
private:
	
};

#endif

