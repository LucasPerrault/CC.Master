using System;
using TechTalk.SpecFlow;


namespace Testing.Specflow
{
    [Binding]
    public class StepArgumentTransformations
    {
        private const string NullValue = "<null>";

        [StepArgumentTransformation(@"(with|without)")]
        public bool TransformWithOrWithoutInBoolean(string withOrWithout)
        {
            switch(withOrWithout)
            {
                case "with":
                    return true;
                case "without":
                    return false;
                default:
                    throw new ArgumentException($"Unknown value {withOrWithout} (only 'with' or 'without' are allowed)");
            }
        }

        [StepArgumentTransformation()]
        public DateTime? TransformDateTimeOrNullToNullableDateTime(string dateTimeOrNull)
        {
            if (dateTimeOrNull == NullValue) { return null; }

            return DateTime.Parse(dateTimeOrNull);
        }

        [StepArgumentTransformation()]
        public string TransformNullValueToNullString(string possibleNullValue)
        {
            if (possibleNullValue == NullValue) { return null; }

            return possibleNullValue;
        }

    }
}
