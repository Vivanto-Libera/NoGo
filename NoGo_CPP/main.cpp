#include"Board.h"
PYBIND11_MODULE(NoGo, m)
{
	py::enum_<Color>(m, "Color")
		.value("EMPTY", Color::EMPTY)
		.value("BLACK", Color::BLACK)
		.value("WHITE", Color::WHITE)
		.export_values();

	py::class_<Board>(m, "Board")
		.def(py::init<>())
		.def(py::init<const Board&>())
		.def("isTerminal", &Board::isTerminal)
		.def("legalMoves", &Board::legalMoves)
		.def("neuralworkInput", &Board::netwrokInput)
		.def("applyMove", &Board::applyMove)
		.def_readwrite("board", &Board::board)
		.def_readwrite("turn", &Board::turn);
		
}