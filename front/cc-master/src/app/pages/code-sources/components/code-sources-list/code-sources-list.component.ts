import { Component, Input } from '@angular/core';

import { getLifecycleStepName, LifecycleStep } from '../../constants/lifecycle-step.enum';
import { ICodeSource } from '../../models/code-source.interface';

@Component({
  selector: 'cc-code-sources-list',
  templateUrl: './code-sources-list.component.html',
  styleUrls: ['./code-sources-list.component.scss'],
})
export class CodeSourcesListComponent {
  @Input() public canEditCodeSources: boolean;
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
    const domain = codeSource.config.isPrivate ? 'lucca.local' : 'ilucca.net';

    return `${ subdomain }.${ domain }${ appPath }`;
  }

  public accessGithub(codeSource: ICodeSource): void {
    window.open(codeSource.githubRepo);
  }
}
