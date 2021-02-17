import { Component, forwardRef } from '@angular/core';
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
  public onChange: (userIds: IPrincipal[]) => void;
  public onTouch: () => void;

  public userIds: IPrincipal[];

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(userIdsUpdated: IPrincipal[]): void {
    if (userIdsUpdated !== this.userIds) {
      this.userIds = userIdsUpdated;
    }
  }

  public safeOnChange(userIdsUpdated: IPrincipal[]): void {
    if (!userIdsUpdated) {
      this.reset();
      return;
    }

    this.onChange(userIdsUpdated);
  }

  public trackBy(index: number, user: IPrincipal): string {
    return user.name;
  }

  private reset(): void {
    this.onChange([]);
  }
}
