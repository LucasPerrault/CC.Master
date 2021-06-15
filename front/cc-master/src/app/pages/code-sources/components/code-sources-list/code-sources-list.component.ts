import { Component, Input } from '@angular/core';

import { CodeSourceType } from '../../constants/code-source-type.enum';
import { getLifecycleStepName, LifecycleStep } from '../../constants/lifecycle-step.enum';
import { ICodeSource } from '../../models/code-source.interface';

@Component({
  selector: 'cc-code-sources-list',
  templateUrl: './code-sources-list.component.html',
})
export class CodeSourcesListComponent {
  @Input() public codeSources: ICodeSource[];

  private readonly githubUrl = 'https://github.com/';

  public getRepoName(repoUrl: string): string {
    return repoUrl.replace(this.githubUrl, '');
  }

  public getLifecycleName(enumValue: LifecycleStep): string {
    return getLifecycleStepName(enumValue);
  }

  public getSubdomainAndAppPath(codeSource: ICodeSource): string {
    const subdomain = codeSource.config.subdomain ?? '{tenant}';
    const appPath = codeSource.config.appPath ?? '';

    return `${ subdomain }.ilucca.net${ appPath }`;
  }

  public isApp(type: CodeSourceType): boolean {
    return type === CodeSourceType.App;
  }

  public accessGithub(codeSource: ICodeSource): void {
    window.open(codeSource.githubRepo);
  }

}
