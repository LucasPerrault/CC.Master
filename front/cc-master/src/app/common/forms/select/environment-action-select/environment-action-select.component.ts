import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { environmentActions, IEnvironmentAction } from '@cc/domain/environments';

@Component({
  selector: 'cc-environment-action-select',
  templateUrl: './environment-action-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EnvironmentActionSelectComponent),
      multi: true,
    },
  ],
})
export class EnvironmentActionSelectComponent implements ControlValueAccessor {
  public onChange: (actionIds: IEnvironmentAction[]) => void;
  public onTouch: () => void;

  public actionIdsSelected: IEnvironmentAction[];

  public get actions(): IEnvironmentAction[] {
    return environmentActions;
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(actionIdsSelectionUpdated: IEnvironmentAction[]): void {
    if (actionIdsSelectionUpdated !== this.actionIdsSelected) {
      this.actionIdsSelected = actionIdsSelectionUpdated;
    }
  }

  public safeOnChange(actionIdsSelectionUpdated: IEnvironmentAction[]): void {
    if (!actionIdsSelectionUpdated) {
      this.reset();
      return;
    }

    this.onChange(actionIdsSelectionUpdated);
  }

  public searchFn(action: IEnvironmentAction, clue: string): boolean {
    return action.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, action: IEnvironmentAction): string {
    return action.name;
  }

  private reset(): void {
    this.onChange([]);
  }
}
