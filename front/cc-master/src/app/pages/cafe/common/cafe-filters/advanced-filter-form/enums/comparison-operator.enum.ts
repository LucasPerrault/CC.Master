export enum ComparisonOperator {
  Equals = 'Equals',
  DoesNotEqual = 'DoesNotEqual',
  Contains = 'Contains',
  DoesNotContain = 'DoesNotContain',
  Between = 'Between',
  StartsWith = 'StartsWith',
  TrueOnly = 'TrueOnly',
  FalseOnly = 'FalseOnly',
  ByPass = 'ByPass',
  Until = 'Until',
  Since = 'Since',
  GreaterThan = 'GreaterThan',
  SmallerThan = 'SmallerThan',
}

export const getComparisonBooleanValue = (operator: ComparisonOperator) => {
  switch (operator) {
    case ComparisonOperator.TrueOnly:
      return 'true';
    case ComparisonOperator.FalseOnly:
      return 'false';
    case ComparisonOperator.ByPass:
      return 'true,false';
  }
};


