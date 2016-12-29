﻿using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using O2DESNet.Optimizer.Samplings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O2DESNet.Optimizer
{
    class TestMultiSphere
    {
        static void Main(string[] args)
        {
            int length = 500, nSeeds = 100;
            Experiment(length, nSeeds, MoCompass.SamplingScheme.CoordinateSampling, pivotSelectionScheme: MoCompass.PivotSelectionScheme.Uniform);
            Experiment(length, nSeeds, MoCompass.SamplingScheme.PolarUniform, pivotSelectionScheme: MoCompass.PivotSelectionScheme.Uniform);

            Experiment(length, nSeeds, MoCompass.SamplingScheme.GoCS, MoCompass.MultiGradientScheme.Unified, MoCompass.PivotSelectionScheme.Uniform);
            Experiment(length, nSeeds, MoCompass.SamplingScheme.GoCS, MoCompass.MultiGradientScheme.Unified, MoCompass.PivotSelectionScheme.MultiGradient);

            Experiment(length, nSeeds, MoCompass.SamplingScheme.GoPolars, MoCompass.MultiGradientScheme.Unified, MoCompass.PivotSelectionScheme.Uniform);
            Experiment(length, nSeeds, MoCompass.SamplingScheme.GoPolars, MoCompass.MultiGradientScheme.Unified, MoCompass.PivotSelectionScheme.MultiGradient);

            //Experiment(length, nSeeds, MoCompass.SamplingScheme.PolarUniform);
            //Experiment(length, nSeeds, MoCompass.SamplingScheme.GoPolars, MoCompass.MultiGradientScheme.Unified);
            //Experiment(length, nSeeds, MoCompass.SamplingScheme.GoPolars, MoCompass.MultiGradientScheme.Averaged);
            //Experiment(length, nSeeds, MoCompass.SamplingScheme.GoPolars, MoCompass.MultiGradientScheme.Random);
        }    
        
        static void Experiment(int length, int nSeeds, 
            MoCompass.SamplingScheme samplingScheme, 
            MoCompass.MultiGradientScheme multiGradientScheme = MoCompass.MultiGradientScheme.Unified,
            MoCompass.PivotSelectionScheme pivotSelectionScheme = MoCompass.PivotSelectionScheme.Uniform)
        {
            int dimension = 5;
            int batchSize = 4;
            var pb = new Benchmarks.MultiSphere(new DenseVector[] {
                new double[] { 0.2, 0.2, 0.2, 0.2, 0.2 }, new double[] { 0.3, 0.4, 0.2, 0.1, 0.3 }, new double[] { 0.1, 0.3, 0.1, 0.4, 0.2 } });

            var stats = new AverageRunningStats(nSeeds);
            Parallel.For(0, nSeeds, seed =>
            {
                var rs = new Random(seed);
                var mocompass = new MoCompass(pb.DecisionSpace, seed: rs.Next());
                mocompass.SamplingSchemeOption = samplingScheme;
                mocompass.MultiGradientSchemeOption = multiGradientScheme;
                mocompass.PivotSelectionSchemeOption = pivotSelectionScheme;

                while (mocompass.AllSolutions.Count < length)
                {
                    DenseVector[] decs;
                    if (mocompass.ParetoSet.Length > 0) decs = mocompass.Sample(batchSize, decimals: 2, nTrials: 10000);
                    else decs = Enumerable.Range(0, batchSize).Select(i => (DenseVector)Enumerable.Range(0, dimension).Select(j => rs.NextDouble()).ToArray()).ToArray(); // randomize initial solutions
                    if (decs.Length < 1) break;
                    mocompass.Enter(decs.Select(d =>
                    {
                        var sol = new StochasticSolution(d, pb.Evaluate(d));
                        sol.Gradients = pb.Gradients(d);
                        return sol;
                    }));
                    lock (stats)
                    {
                        stats.Log(seed, mocompass.AllSolutions.Count,
                            Pareto.DominatedHyperVolume(mocompass.ParetoSet.Select(s => s.Objectives), new double[] { 5, 5, 5 }));
                    }
                    //Console.Clear();
                    Console.WriteLine("Seed: {0}, #Samples: {1}", seed, mocompass.AllSolutions.Count);
                }
            });

            using (var sw = new System.IO.StreamWriter(
                string.Format("results_{0}_{1}_{2}_{3}.csv", pb, samplingScheme, multiGradientScheme, pivotSelectionScheme)))
                foreach (var t in stats.Output) sw.WriteLine("{0},{1}", t.Item1, t.Item2);
        }    
    }
}