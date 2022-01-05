const conf = require('rc')('lu-proxy');

const PROXY_CONFIG = [
	{
		context: [
			'/api',
			'/ip-filter'
		],
		logLevel: 'debug',
    changeOrigin: true,
    secure: false,
		target: `https://${conf.url}`,
		headers: {
      Authorization: `cloudcontrol user=${conf.token}`,
    },
	}
];

module.exports = PROXY_CONFIG;
