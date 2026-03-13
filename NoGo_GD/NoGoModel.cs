using System;
using System.Collections.Generic;
using System.Linq;
using static TorchSharp.torch;
using TorchSharp.Modules;
using TorchSharp;

namespace NoGO
{
    public class NoGoModel
    {
        jit.ScriptModule model;
        public ValueTuple<Tensor, Tensor> Predict(Tensor input)
        {
            return (ValueTuple<Tensor, Tensor>)model.forward(input);
        }
        public NoGoModel()
        {
            model = jit.load("Baduk.pt");
            model.eval();
        }
    }
}

