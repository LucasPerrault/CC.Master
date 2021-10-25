import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NavigationPath } from '@cc/common/navigation';
import { IEnvironment } from '@cc/domain/environments';

@Component({
  selector: 'cc-contract-environment-linked',
  templateUrl: './contract-environment-linked.component.html',
})
export class EnvironmentLinkedInformationComponent {
  @Input() public environment: IEnvironment | null;
  @Input() public canRemoveEnvironmentLinked: boolean;
  @Input() public unlinkButtonState: string;
  @Output() public unlinkEnvironment: EventEmitter<void> = new EventEmitter<void>();

  constructor() { }

  public getEnvironmentName(environment: IEnvironment): string {
    return `${ environment.subDomain }.${ environment.domainName }`;
  }

  public redirectToEnvironment(environment: IEnvironment): void {
    const routerParams = `main-id=${environment.id}&ccselect-tags=(id~${environment.id})`;
    const redirectionUrl = `${ NavigationPath.Environments }#!?${ routerParams }`;
    window.open(redirectionUrl);
  }

  public unlink(): void {
    this.unlinkEnvironment.emit();
  }
}
