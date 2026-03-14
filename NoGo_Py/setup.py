from distutils.core import setup
from distutils.extension import Extension
from Cython.Build import cythonize
import numpy
ext = Extension(
        'mcts',['mcts.pyx'],include_dirs=[numpy.get_include()]
    )
setup(name='mcts',ext_modules = [ext])
