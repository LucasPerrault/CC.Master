import { AutoSelectedColumnMappingHelper, IAutoSelectedColumnMapping } from '../../common/services/column-auto-selection';
import { EstablishmentCriterionKey } from '../advanced-filter/establishment-criterion-key.enum';
import { EstablishmentAdditionalColumn, getColumnById } from './establishment-additional-column';

export const etsAutoSelectedColumnMapping: IAutoSelectedColumnMapping[] = [
  {
    criterionKey: EstablishmentCriterionKey.Country,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EstablishmentAdditionalColumn.Country, getColumnById),
  },
  {
    criterionKey: EstablishmentCriterionKey.Environment,
    getColumn: () => AutoSelectedColumnMappingHelper.getAdditionalColumn(EstablishmentAdditionalColumn.Environment, getColumnById),
  },
  {
    criterionKey: EstablishmentCriterionKey.Facet,
    getColumn: AutoSelectedColumnMappingHelper.getFacetColumn,
  },
];

