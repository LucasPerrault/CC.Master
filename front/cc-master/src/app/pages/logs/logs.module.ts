import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LogsComponent } from './logs.component';
import {TranslateModule} from '@cc/aspects/translate';

const routes: Routes = [
	{ path: '', component: LogsComponent },
];

@NgModule({
	declarations: [
		LogsComponent,
	],
    imports: [
        RouterModule.forChild(routes),
        TranslateModule,
    ]
})
export class LogsModule {}
