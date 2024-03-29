﻿namespace NinjaBay.Shared.ValueObjects
{
    public class QuestionAnswer
    {
        public string Question { get; set; }

        public string Answer { get; set; }

        public void Update(QuestionAnswer questionAnswer)
        {
            Question = string.IsNullOrEmpty(questionAnswer.Question) ? Question : questionAnswer.Question;
            Answer = string.IsNullOrEmpty(questionAnswer.Answer) ? Answer : questionAnswer.Answer;
        }
    }
}