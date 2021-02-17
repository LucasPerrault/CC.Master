import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { environmentDomains, IEnvironmentDomain } from '@cc/domain/environments';

@Component({
  selector: 'cc-environment-domain-select',
  templateUrl: './environment-domain-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EnvironmentDomainSelectComponent),
      multi: true,
    },
  ],
})
export class EnvironmentDomainSelectComponent implements ControlValueAccessor {
  public onChange: (domainIds: IEnvironmentDomain[]) => void;
  public onTouch: () => void;

  public domainIdsSelected: IEnvironmentDomain[];
  public get environmentDomains(): IEnvironmentDomain[] {
    return environmentDomains;
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(domainIdsSelectionUpdated: IEnvironmentDomain[]): void {
    if (domainIdsSelectionUpdated !== this.domainIdsSelected) {
      this.domainIdsSelected = domainIdsSelectionUpdated;
    }
  }

  public safeOnChange(domainIdsSelectionUpdated: IEnvironmentDomain[]): void {
    if (!domainIdsSelectionUpdated) {
      this.reset();
      return;
    }

    this.onChange(domainIdsSelectionUpdated);
  }

  public searchFn(domain: IEnvironmentDomain, clue: string): boolean {
    return domain.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, domain: IEnvironmentDomain): string {
    return domain.name;
  }

  private reset(): void {
    this.onChange([]);
  }
}
