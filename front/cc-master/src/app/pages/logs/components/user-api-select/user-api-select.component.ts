import { Component, EventEmitter, Output } from '@angular/core';
import { IPrincipal } from '@cc/aspects/principal';
import { ALuApiService } from '@lucca-front/ng/api';

import { UserApiSelectService } from './user-api-select.service';

@Component({
  selector: 'cc-user-api-select',
  templateUrl: './user-api-select.component.html',
  providers: [{
    provide: ALuApiService, useClass: UserApiSelectService,
  }],
})
export class UserApiSelectComponent {
  @Output() public userIdsToString: EventEmitter<string> = new EventEmitter<string>();
  public userIds: number[];

  public trackBy(index: number, user: IPrincipal): string {
    return user.name;
  }

  public updateUserIdsSelected(userIds: number[]): void {
    if (!userIds) {
      this.userIdsToString.emit('');
      return;
    }

    const userIdsToString = userIds.join(',');
    this.userIdsToString.emit(userIdsToString);
  }
}
