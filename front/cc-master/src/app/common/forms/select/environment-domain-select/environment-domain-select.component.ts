import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { environmentDomains, IEnvironmentDomain } from '@cc/domain/environments';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

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
  @Input() public placeholder: string;
  @Input() public formlyAttributes: FormlyFieldConfig = {};
  @Input() public multiple = false;

  public onChange: (domainIds: IEnvironmentDomain[]) => void;
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

  public trackBy(index: number, domain: IEnvironmentDomain): number {
    return domain.id;
  }

  public getDomainNamesRawString(domains: IEnvironmentDomain[]): string {
    return domains.map(d => d.name).join(', ');
  }

  private reset(): void {
    this.onChange([]);
  }
}
