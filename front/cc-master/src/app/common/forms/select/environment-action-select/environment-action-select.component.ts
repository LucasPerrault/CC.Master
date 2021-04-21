import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
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
  @Input() public class?: string;

  public onChange: (actionIds: IEnvironmentAction[]) => void;
  public onTouch: () => void;

  public actions: IEnvironmentAction[];
  public actionsSelected: IEnvironmentAction[];

  constructor(private translatePipe: TranslatePipe) {
    this.actions = this.setTranslatedActions(environmentActions);
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(actionsSelectionUpdated: IEnvironmentAction[]): void {
    if (!!actionsSelectionUpdated && actionsSelectionUpdated !== this.actionsSelected) {
      this.actionsSelected = this.setTranslatedActions(actionsSelectionUpdated);
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

  public sort(actions: IEnvironmentAction[]): void {
    const actionsSortedByName = this.orderByName(actions);
    this.actions = this.orderBySelection(actionsSortedByName);
  }

  private setTranslatedActions(actions: IEnvironmentAction[]): IEnvironmentAction[] {
    return actions.map(a => ({
      ...a,
      name: this.translatePipe.transform(a.name),
    }));
  }

  private orderByName(actions: IEnvironmentAction[]): IEnvironmentAction[] {
    return actions.sort((a, b) => a.name.localeCompare(b.name));
  }

  private orderBySelection(actions: IEnvironmentAction[]): IEnvironmentAction[] {
    if (!this.actionsSelected || !this.actionsSelected.length) {
      return actions;
    }

    const actionSelectedIds = this.actionsSelected.map(a => a.id);
    const actionsNotSelected = actions.filter(a => !actionSelectedIds.includes(a.id));
    return [...this.actionsSelected, ...actionsNotSelected];
  }

  private reset(): void {
    this.onChange([]);
  }
}
