import { CodeSourceType } from '../constants/code-source-type.enum';
import { LifecycleStep } from '../constants/lifecycle-step.enum';
import { ICodeSourceConfig } from './code-source-config.interface';

export interface ICodeSource {
  id: number;
  name: string;
  code: string;
  type: CodeSourceType;
  githubRepo: string;
  lifecycle: LifecycleStep;
  jenkinsProjectName: string;
  JenkinsProjectUrl: string;
  config: ICodeSourceConfig;
}
