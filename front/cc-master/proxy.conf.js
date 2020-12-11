const luccaProxy = require('@lucca/proxy');

const PROXY_CONFIG = [
	luccaProxy({
		context: [
			'/api',
		],
		logLevel: 'debug',
	}),
];

module.exports = PROXY_CONFIG;
