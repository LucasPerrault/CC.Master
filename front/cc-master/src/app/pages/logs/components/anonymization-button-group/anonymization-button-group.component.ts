import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'cc-anonymization-button-group',
  templateUrl: './anonymization-button-group.component.html',
})
export class AnonymizationButtonGroupComponent implements OnInit {
  @Output() public isAnonymizedToString: EventEmitter<string> = new EventEmitter<string>();

  public isAnonymizedData = '';
  private routerParamKey = 'isAnonymized';

  constructor(private router: Router, private activatedRoute: ActivatedRoute ) {
  }

  ngOnInit() {
    const routerParamValue = this.activatedRoute.snapshot.queryParamMap.get(this.routerParamKey);
    if (!routerParamValue) {
      return;
    }

    this.isAnonymizedToString.emit(routerParamValue);
    this.isAnonymizedData = routerParamValue;
  }

  public async updateAsync(isAnonymizedToString: string) {
    this.isAnonymizedToString.emit(isAnonymizedToString);
    await this.updateRouterAsync(isAnonymizedToString);
  }

  private async updateRouterAsync(value: string): Promise<void> {
    const queryParams = { [this.routerParamKey]: !!value ? value : null };

    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams,
      queryParamsHandling: 'merge',
    });
  }
}
