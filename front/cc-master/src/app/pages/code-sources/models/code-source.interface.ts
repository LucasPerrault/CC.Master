import { CodeSourceType } from '../constants/code-source-type.enum';
import { LifecycleStep } from '../constants/lifecycle-step.enum';
import { ICodeSourceConfig } from './code-source-config.interface';
import { IGithubRepo } from './repos.interface';

export interface ICodeSource {
  id: number;
  name: string;
  code: string;
  type: CodeSourceType;
  lifecycle: LifecycleStep;
  jenkinsProjectName: string;
  jenkinsProjectUrl: string;
  config: ICodeSourceConfig;
  repo: IGithubRepo;
}
