﻿// This file was auto-generated by ML.NET Model Builder. 
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace MLModel3_PredictSatisfy
{
    public partial class MLModelPredictReason
    {
        /// <summary>
        /// model input class for MLModelPredictReason.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [ColumnName(@"Total score")]
            public float Total_score { get; set; }

            [ColumnName(@"Reason")]
            public string Reason { get; set; }

            [ColumnName(@"Company culture")]
            public float Company_culture { get; set; }

            [ColumnName(@"Job satisfaction")]
            public float Job_satisfaction { get; set; }

            [ColumnName(@"Professional growth")]
            public float Professional_growth { get; set; }

            [ColumnName(@"Manager relationship")]
            public float Manager_relationship { get; set; }

            [ColumnName(@"Compensation and benefits")]
            public float Compensation_and_benefits { get; set; }

            [ColumnName(@"Work-life balance")]
            public float Work_life_balance { get; set; }

            [ColumnName(@"Satisfy?")]
            public bool Satisfy_ { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for MLModelPredictReason.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"Total score")]
            public float Total_score { get; set; }

            [ColumnName(@"Reason")]
            public uint Reason { get; set; }

            [ColumnName(@"Company culture")]
            public float Company_culture { get; set; }

            [ColumnName(@"Job satisfaction")]
            public float Job_satisfaction { get; set; }

            [ColumnName(@"Professional growth")]
            public float Professional_growth { get; set; }

            [ColumnName(@"Manager relationship")]
            public float Manager_relationship { get; set; }

            [ColumnName(@"Compensation and benefits")]
            public float Compensation_and_benefits { get; set; }

            [ColumnName(@"Work-life balance")]
            public float Work_life_balance { get; set; }

            [ColumnName(@"Satisfy?")]
            public bool Satisfy_ { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"PredictedLabel")]
            public string PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float[] Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("MLModelPredictReason.zip");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}
