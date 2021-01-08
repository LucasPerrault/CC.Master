import { Component, EventEmitter, Output } from '@angular/core';

import { environmentActions } from '../../enums';
import { IEnvironmentAction } from '../../models';

@Component({
  selector: 'cc-environment-action-select',
  templateUrl: './environment-action-select.component.html',
})
export class EnvironmentActionSelectComponent {
  @Output() public actionIdsToString: EventEmitter<string> = new EventEmitter<string>();

  public actionIds: number[];

  public searchFn(action: IEnvironmentAction, clue: string): boolean {
    return action.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, activity: IEnvironmentAction): string {
    return activity.name;
  }

  public updateActionIdsSelected(actionIds: number[]): void {
    if (!actionIds) {
      return;
    }
    const actionIdsToString = actionIds.join(',');
    this.actionIdsToString.emit(actionIdsToString);
  }


  public get actions(): IEnvironmentAction[] {
    return environmentActions;
  }

}
