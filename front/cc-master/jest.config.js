module.exports = {
  preset: 'jest-preset-angular',
  testURL: 'https://localhost:4200',
  setupFiles: ['<rootDir>/node_modules/core-js'],
  setupFilesAfterEnv: ['<rootDir>/setupJest.ts'],
  globals: {
    'ts-jest': {
      stringifyContentPathRegex: '\\.html?$',
      tsconfig: 'tsconfig.spec.json',
      astTransformers: {
        before: ['jest-preset-angular/build/InlineFilesTransformer', 'jest-preset-angular/build/StripStylesTransformer'],
      },
    },
  },
  testPathIgnorePatterns: ['<rootDir>/node_modules/'],
  modulePathIgnorePatterns: ['<rootDir>/dist'],
  moduleNameMapper: {
    "@cc/(.*)": "<rootDir>/src/app/$1"
  },
  coverageDirectory: 'results',
  collectCoverageFrom: ['<rootDir>/src/app/**/*.ts', '!<rootDir>/src/app/**/*.module.ts', '!<rootDir>/src/app/**/index.ts'],
  coveragePathIgnorePatterns: ['/node_modules/', 'translations.ts'],
};
