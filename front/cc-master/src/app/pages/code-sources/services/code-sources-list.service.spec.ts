import { createServiceFactory, mockProvider, SpectatorService } from '@ngneat/spectator';
import { BehaviorSubject } from 'rxjs';

import { CodeSourceType } from '../constants/code-source-type.enum';
import { LifecycleStep } from '../constants/lifecycle-step.enum';
import { ICodeSource } from '../models/code-source.interface';
import { CodeSourcesService } from './code-sources.service';
import { CodeSourcesListService } from './code-sources-list.service';

const fakeCodeSource = (lifecycle: LifecycleStep) => ({
  id: 1,
  name: 'CC',
  code: 'CC',
  jenkinsProjectName: 'CC',
  jenkinsProjectUrl: '******',
  type: CodeSourceType.InternalTool,
  repo: {
    id: 2,
    name: "name",
    url: "*****"
  },
  lifecycle,
  config: {
    appPath: '',
    subdomain: 'cc',
    iisServerPath: '******',
    isPrivate: true,
  },
});
const mockCodeSources$: BehaviorSubject<ICodeSource[]> = new BehaviorSubject<ICodeSource[]>([
  fakeCodeSource(LifecycleStep.Referenced),
  fakeCodeSource(LifecycleStep.Preview),
  fakeCodeSource(LifecycleStep.Deleted),
  fakeCodeSource(LifecycleStep.InProduction),
  fakeCodeSource(LifecycleStep.ReadyForDeploy),
  fakeCodeSource(LifecycleStep.ToDelete),
]);

describe('CodeSourcesListService', () => {
  let spectator: SpectatorService<CodeSourcesListService>;
  const createService = createServiceFactory({
    providers: [mockProvider(CodeSourcesService, { getCodeSources$: () => mockCodeSources$ })],
    service: CodeSourcesListService,
  });

  beforeEach(() => spectator = createService());

  it('should only get referenced code sources ', () => {
    spectator.service.updateFilters(LifecycleStep.Referenced);

    let result: ICodeSource[] = [];
    spectator.service.codeSources$.subscribe(sources => result = sources);

    const referencedLifecycle = LifecycleStep.Referenced;
    expect(result.every(s => s.lifecycle === referencedLifecycle)).toBeTruthy();
  });

  it('should only get deleted or to deleted code sources ', () => {
    spectator.service.updateFilters(LifecycleStep.Deleted);

    let result: ICodeSource[] = [];
    spectator.service.codeSources$.subscribe(sources => result = sources);

    const deletedLifecycles = LifecycleStep.ToDelete || LifecycleStep.Deleted;
    expect(result.every(s => s.lifecycle === deletedLifecycles)).toBeTruthy();
  });

  it('should only get active code sources ', () => {
    spectator.service.updateFilters(LifecycleStep.Preview);

    let result: ICodeSource[] = [];
    spectator.service.codeSources$.subscribe(sources => result = sources);

    const activeLifecycles = LifecycleStep.Preview || LifecycleStep.ReadyForDeploy || LifecycleStep.InProduction;
    expect(result.every(s => s.lifecycle === activeLifecycles)).toBeTruthy();
  });
});
