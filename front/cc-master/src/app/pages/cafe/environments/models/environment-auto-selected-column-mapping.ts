import { AutoSelectedColumnMappingHelper, IAutoSelectedColumnMapping } from '../../common/services/column-auto-selection';
import { EnvironmentCriterionKey } from '../advanced-filter/environment-criterion-key.enum';
import { EnvironmentAdditionalColumn, getColumnById } from './environment-additional-column';

export const envAutoSelectedColumnMapping: IAutoSelectedColumnMapping[] = [
  {
    criterionKey: EnvironmentCriterionKey.AppInstances,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EnvironmentAdditionalColumn.Environment, getColumnById),
  },
  {
    criterionKey: EnvironmentCriterionKey.Distributors,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EnvironmentAdditionalColumn.Distributors, getColumnById),
  },
  {
    criterionKey: EnvironmentCriterionKey.CreatedAt,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EnvironmentAdditionalColumn.CreatedAt, getColumnById),
  },
  {
    criterionKey: EnvironmentCriterionKey.Countries,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EnvironmentAdditionalColumn.Countries, getColumnById),
  },
  {
    criterionKey: EnvironmentCriterionKey.Cluster,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EnvironmentAdditionalColumn.Cluster, getColumnById),
  },
  {
    criterionKey: EnvironmentCriterionKey.DistributorType,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EnvironmentAdditionalColumn.DistributorType, getColumnById),
  },
  {
    criterionKey: EnvironmentCriterionKey.Facets,
    getColumn: AutoSelectedColumnMappingHelper.getFacetColumn,
  },
];

