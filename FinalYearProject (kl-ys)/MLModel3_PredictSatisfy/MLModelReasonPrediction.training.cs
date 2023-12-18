﻿﻿// This file was auto-generated by ML.NET Model Builder. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers;
using Microsoft.ML;

namespace MLModel3_PredictSatisfy
{
    public partial class MLModelReasonPrediction
    {
        /// <summary>
        /// Retrains model using the pipeline generated as part of the training process. For more information on how to load data, see aka.ms/loaddata.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        public static ITransformer RetrainPipeline(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"Company culture", @"Company culture"),new InputOutputColumnPair(@"Job satisfaction", @"Job satisfaction"),new InputOutputColumnPair(@"Professional growth", @"Professional growth"),new InputOutputColumnPair(@"Manager relationship", @"Manager relationship"),new InputOutputColumnPair(@"Compensation and benefits", @"Compensation and benefits"),new InputOutputColumnPair(@"Work-life balance", @"Work-life balance")})      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"Company culture",@"Job satisfaction",@"Professional growth",@"Manager relationship",@"Compensation and benefits",@"Work-life balance"}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"Reason",inputColumnName:@"Reason"))      
                                    .Append(mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator:mlContext.BinaryClassification.Trainers.FastTree(new FastTreeBinaryTrainer.Options(){NumberOfLeaves=4,MinimumExampleCountPerLeaf=20,NumberOfTrees=4,MaximumBinCountPerFeature=255,FeatureFraction=1,LearningRate=0.1,LabelColumnName=@"Reason",FeatureColumnName=@"Features"}),labelColumnName: @"Reason"))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
}