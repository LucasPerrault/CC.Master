import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LogsComponent } from './logs.component';

const routes: Routes = [
	{ path: '', component: LogsComponent }
]

@NgModule({
	declarations: [
		LogsComponent
	],
	imports: [
		RouterModule.forChild(routes),
	]
})
export class LogsModule {}