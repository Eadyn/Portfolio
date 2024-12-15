using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.ML.Data;


namespace BookRecommender
{
    public partial class BookRecommenderBase : ComponentBase
    {

        MLContext mlContext;
        List<TextData> emptySamples;
        IDataView emptyDataView;

        EstimatorChain<WordEmbeddingTransformer> textPipeline;
        ITransformer textTransformer;
        private PredictionEngine<TextData, TransformedTextData> predictionEngine;

        [Parameter] public string book { get; set; } = string.Empty;

        public static List<string> bookList = new List<string>();

        public static List<TransformedTextData> transformedBookList = new List<TransformedTextData>();

        protected override void OnInitialized()
        {
            mlContext = new MLContext();
            emptySamples = new List<TextData>();
            emptyDataView = mlContext.Data.LoadFromEnumerable(emptySamples);
            textPipeline = mlContext.Transforms.Text.NormalizeText("Text")
            .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens",
                "Text"))
            .Append(mlContext.Transforms.Text.ApplyWordEmbedding("Features",
                inputColumnName: "Tokens", customModelFile: "wwwroot/25d.txt"));
            var textTransformer = textPipeline.Fit(emptyDataView);

            predictionEngine = mlContext.Model.CreatePredictionEngine<TextData,
            TransformedTextData>(textTransformer);
        }

        public class TextData
        {
            public string Text { get; set; }
        }

        public class TransformedTextData : TextData
        {
            public float[] Features { get; set; }
        }


        private TransformedTextData TransformText(string text)
        {
            var textData = new TextData { Text = text };
            var transformedData = predictionEngine.Predict(textData);
            return transformedData;
        }

        public void AddBook()
        {
            if (book.Trim() != string.Empty)
            {
                bookList.Add(book);
                transformedBookList.Add(TransformText(book));
                book = string.Empty;
            }
        }

        public void ClearList()
        {
            bookList.Clear();
        }
    }
}