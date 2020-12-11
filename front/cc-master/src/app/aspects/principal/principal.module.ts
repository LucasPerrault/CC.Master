import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, ModuleWithProviders, NgModule } from '@angular/core';
import { getPrincipal, initPrincipal, PrincipalInitializer } from './principal.initializer';
// import { PrincipalService } from './principal.service';
import { PRINCIPAL } from './principal.token';

@NgModule({
	imports: [
		HttpClientModule,
	]
})
export class PrincipalModule {
	public static forRoot(): ModuleWithProviders<PrincipalModule> {
		return {
			ngModule: PrincipalModule,
			providers: [
				PrincipalInitializer,
				// PrincipalService,
				{
					provide: APP_INITIALIZER,
					useFactory: initPrincipal,
					deps: [PrincipalInitializer],
					multi: true
				},
				{
					provide: PRINCIPAL,
					useFactory: getPrincipal,
					deps: [PrincipalInitializer]
				},
			],
		};
	}
}
