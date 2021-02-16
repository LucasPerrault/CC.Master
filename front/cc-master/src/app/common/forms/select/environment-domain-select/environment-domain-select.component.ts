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
  public onChange: (domains: IEnvironmentDomain[]) => void;
  public onTouch: () => void;

  public domainsSelected: IEnvironmentDomain[];
  public get environmentDomains(): IEnvironmentDomain[] {
    return environmentDomains;
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(domainsSelectionUpdated: IEnvironmentDomain[]): void {
    if (domainsSelectionUpdated !== this.domainsSelected) {
      this.domainsSelected = domainsSelectionUpdated;
    }
  }

  public safeOnChange(domainsSelectionUpdated: IEnvironmentDomain[]): void {
    if (!domainsSelectionUpdated) {
      this.reset();
      return;
    }

    this.onChange(domainsSelectionUpdated);
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
