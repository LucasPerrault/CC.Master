import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { UsersService } from './services/users.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ],
  providers: [UsersService],
})
export class UsersModule { }
