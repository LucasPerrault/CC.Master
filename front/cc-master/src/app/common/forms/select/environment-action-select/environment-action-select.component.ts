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

  public actionsSelected: IEnvironmentAction[];

  public get actions(): IEnvironmentAction[] {
    return environmentActions;
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(actionsSelectionUpdated: IEnvironmentAction[]): void {
    if (actionsSelectionUpdated !== this.actionsSelected) {
      this.actionsSelected = actionsSelectionUpdated;
    }
  }

  public safeOnChange(actionsSelectionUpdated: IEnvironmentAction[]): void {
    if (!actionsSelectionUpdated) {
      this.reset();
      return;
    }

    this.onChange(actionsSelectionUpdated);
  }

  public searchFn(action: IEnvironmentAction, clue: string): boolean {
    return action.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, action: IEnvironmentAction): number {
    return action.id;
  }

  public getActionNamesRawString(actions: IEnvironmentAction[]): string {
    return actions.map(a => a.name).join(', ');
  }

  private reset(): void {
    this.onChange([]);
  }
}
