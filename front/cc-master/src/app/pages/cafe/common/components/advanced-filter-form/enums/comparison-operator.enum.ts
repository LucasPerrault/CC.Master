export enum ComparisonOperator {
  Equals = 'Equals',
  NotEquals = 'NotEquals',
  TrueOnly = 'TrueOnly',
  FalseOnly = 'FalseOnly',
  StrictlyGreaterThan = 'StrictlyGreaterThan',
  StrictlyLessThan = 'StrictlyLessThan',
  ListContains = 'Contains',
  ListAreAmong = 'AreAmong',
  ListNotContains = 'NotContains',
  ListContainsOnly = 'ContainsOnly',
  Between = 'Between',
}

export const getComparisonBooleanValue = (operator: ComparisonOperator) => {
  switch (operator) {
    case ComparisonOperator.TrueOnly:
      return true;
    case ComparisonOperator.FalseOnly:
      return false;
  }
};


