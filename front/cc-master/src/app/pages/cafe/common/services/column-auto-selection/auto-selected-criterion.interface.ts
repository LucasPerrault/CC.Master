import { IAdvancedCriterionForm } from '../../components/advanced-filter-form';
import { IAutoSelectedColumnState } from './auto-selected-column-mapping.interface';

export interface IAutoSelectedCriterionForm extends IAdvancedCriterionForm {
  state: IAutoSelectedColumnState;
}
