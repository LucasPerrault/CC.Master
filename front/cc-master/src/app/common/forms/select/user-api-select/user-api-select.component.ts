import { ChangeDetectorRef, Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IPrincipal } from '@cc/aspects/principal';
import { ALuApiService } from '@lucca-front/ng/api';

import { UserApiSelectService } from './user-api-select.service';

@Component({
  selector: 'cc-user-api-select',
  templateUrl: './user-api-select.component.html',
  providers: [
    {
      provide: ALuApiService, useClass: UserApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UserApiSelectComponent),
      multi: true,
    },
  ],
})
export class UserApiSelectComponent implements ControlValueAccessor {
  public onChange: (users: IPrincipal[]) => void;
  public onTouch: () => void;

  public users: IPrincipal[] = [];

  constructor(private changeDetectorRef: ChangeDetectorRef) {
  }


  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(usersUpdated: IPrincipal[]): void {
    if (usersUpdated !== this.users && usersUpdated !== null) {
      this.users = usersUpdated;
      this.changeDetectorRef.detectChanges();
    }
  }

  public safeOnChange(usersUpdated: IPrincipal[]): void {
    if (!usersUpdated) {
      this.reset();
      return;
    }

    this.onChange(usersUpdated);
  }

  public trackBy(index: number, user: IPrincipal): string {
    return user.name;
  }

  public getUserNamesRawString(users: IPrincipal[]): string {
    return users.map(u => u.name).join(', ');
  }

  private reset(): void {
    this.onChange([]);
  }
}
