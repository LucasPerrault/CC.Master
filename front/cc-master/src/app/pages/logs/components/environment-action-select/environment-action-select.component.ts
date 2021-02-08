import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environmentActions, IEnvironmentAction } from '@cc/domain/environments';

@Component({
  selector: 'cc-environment-action-select',
  templateUrl: './environment-action-select.component.html',
})
export class EnvironmentActionSelectComponent implements OnInit {
  @Output() public actionIdsToString: EventEmitter<string> = new EventEmitter<string>();

  public actionIds: number[];
  private routerParamKey = 'actionIds';

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
  }

  public ngOnInit(): void {
    const routerParamValue = this.activatedRoute.snapshot.queryParamMap.get(this.routerParamKey);
    if (!routerParamValue) {
      return;
    }

    this.actionIdsToString.emit(routerParamValue);
    this.actionIds = routerParamValue.split(',').map(i => parseInt(i, 10));
  }

  public searchFn(action: IEnvironmentAction, clue: string): boolean {
    return action.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, activity: IEnvironmentAction): string {
    return activity.name;
  }

  public async updateActionIdsSelectedAsync(actionIds: number[]): Promise<void> {
    if (!actionIds) {
      return;
    }
    const actionIdsToString = actionIds.join(',');
    this.actionIdsToString.emit(actionIdsToString);

    await this.updateRouterAsync(actionIdsToString);
  }

  private async updateRouterAsync(value: string): Promise<void> {
    const queryParams = { [this.routerParamKey]: !!value ? value : null };

    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams,
      queryParamsHandling: 'merge',
    });
  }


  public get actions(): IEnvironmentAction[] {
    return environmentActions;
  }

}
