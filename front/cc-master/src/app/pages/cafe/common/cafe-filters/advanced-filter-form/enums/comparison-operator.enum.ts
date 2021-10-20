export enum ComparisonOperator {
  Contains = 'Contains',
  Equals = 'Equals',
  NotEquals = 'NotEquals',
  TrueOnly = 'TrueOnly',
  FalseOnly = 'FalseOnly',
  StrictlyGreaterThan = 'StrictlyGreaterThan',
  StrictlyLessThan = 'StrictlyLessThan',
}

export const getComparisonBooleanValue = (operator: ComparisonOperator) => {
  switch (operator) {
    case ComparisonOperator.TrueOnly:
      return true;
    case ComparisonOperator.FalseOnly:
      return false;
  }
};


