import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';

import { LogsComponent } from './logs.component';

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
    ],
})
export class LogsModule {}
