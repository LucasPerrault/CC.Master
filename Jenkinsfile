@Library('Lucca@v0.7.1')

import hudson.Util;
import fr.lucca.CI;

ciBuildProperties(script: this);

node(label: CI.getSelectedNode(script: this)) {
	notifyStartStats()
	/////////////////////////////////////////////////
	// Variables à ajuster pour le projet		  //
	/////////////////////////////////////////////////

	// Utilisé à différents endroits (pour générer des noms de dossier par exemple)
	// Ne pas mettre d'espaces ou de caractères bizarres
	def slackChannel = "#cc-ci-cd"
	def projectTechnicalName = "CC.Master"
	def slnFilepath = "back\\CC.Master.sln"
	def repoName = "CC.Master"
	// def nodeJsVersion = "Node LTS v12.x.y"    -- uncomment when front is migrated

	/////////////////////////////////////////////////
	// Fin des variables à ajuster pour le projet //
	/////////////////////////////////////////////////

	def prepareDirectory = '.prepare';
	def buildDirectory = '.build';
	def archiveDirectory = '.jenkins';

	def isPr = false
	def isMainBranch = false
	if(env.BRANCH_NAME == "master" || env.BRANCH_NAME == "rc") {
		isMainBranch = true
	}
	if(env.BRANCH_NAME ==~ /^PR-\d*/) {
		isPr = true
	}

	def scmVars = null
	def sonarHostUrl = "https://sonarcloud.io"
	def sonarRunner = "dotnet-sonarscanner"
	def coverage = "coverage"
	def sonarCoverage = "${WORKSPACE}\\${coverage}\\SonarQube.xml"

	try {
		timeout(time: 15, unit: 'MINUTES') {

			def prefix = ""
			def suffix = ""
			def semver = ""

			loggableStage('Notify') {
				// echo
				echo "project ${projectTechnicalName}"
				echo "branch ${env.BRANCH_NAME}"
				echo "slave ${env.NODE_NAME}"

				slackBuildStart channel: slackChannel
			}
			loggableStage('1. Cleanup') {
				if(fileExists(prepareDirectory)) {
					dir(prepareDirectory) {
						deleteDir()
					}
				}
				if(fileExists(buildDirectory)) {
					dir(buildDirectory) {
						deleteDir()
					}
				}
				if(fileExists(archiveDirectory)) {
					dir(archiveDirectory) {
						deleteDir()
					}
				}
			}


			loggableStage('2. Prepare') {
				// git
				scmVars = checkout scm
				slackBuildStartUpdate scmVars: scmVars

				// // semver
				bat "set \"IGNORE_NORMALISATION_GIT_HEAD_MOVE=1\" & dotnet gitversion /output buildserver"
				def gitVersionProps = readProperties file: 'gitversion.properties'
				prefix = gitVersionProps["GitVersion_MajorMinorPatch"]
				suffix = gitVersionProps["GitVersion_NuGetPreReleaseTagV2"]
				semver = gitVersionProps["GitVersion_AssemblySemVer"]
				echo "Version calculated : ---------> prefix: ${prefix}  suffix: ${suffix}  semver: ${semver} "

				// lucca
				bat "dotnet tool install --no-cache --tool-path=${WORKSPACE}\\${prepareDirectory} --add-source http://nuget.lucca.local/nuget devtools"
				env.PATH="${WORKSPACE}\\${prepareDirectory};${env.PATH}"
				bat "lucca --version"

			}
			loggableStage('3. Restore') {
				// back
				bat "dotnet clean ${slnFilepath}"
				bat "dotnet restore ${slnFilepath}"
			}

			if(CI.isSonarEnabled(script:this, extraCondition: isPr || isMainBranch)) {
				loggableStage('4. Qualif') {
					// sonar start
					withCredentials([string(credentialsId: 'Sonarcloud', variable: 'Sonarcloud')]) {
							def sonarArgs = ""
							if(isPr){
									sonarArgs = " /d:\"sonar.pullrequest.key=%CHANGE_ID%\" /d:\"sonar.pullrequest.branch=%CHANGE_BRANCH%\" /d:\"sonar.pullrequest.base=%CHANGE_TARGET%\" /d:sonar.pullrequest.provider=GitHub "
							}
							def sonarRunnerCommand = "${sonarRunner} begin /o:lucca /k:\"${projectTechnicalName}\" /v:\"${semver}\" /d:\"sonar.host.url=${sonarHostUrl}\" /d:\"sonar.login=%Sonarcloud%\" /d:\"sonar.pullrequest.github.repository=LuccaSA/${repoName}\" ${sonarArgs} /d:sonar.coverageReportPaths=\"${sonarCoverage}\" "
							echo "bat : ${sonarRunnerCommand}"
							bat "${sonarRunnerCommand}"
					}

					// back
					bat "if exist ${coverage} rmdir /s /q ${coverage}"
					bat "dotnet test ${slnFilepath} /p:Platform=\"Any CPU\" /nodereuse:false /p:DebugType=Full /p:Configuration=Debug --collect:\"XPlat Code Coverage\" --settings coverlet.runsettings --results-directory:\"${WORKSPACE}\\${coverage}\""
					bat "reportgenerator \"-reports:${coverage}\\*\\*.xml\" \"-targetdir:${WORKSPACE}\\${coverage}\" -reporttypes:SonarQube"

					// sonar end
					withCredentials([string(credentialsId: 'Sonarcloud', variable: 'Sonarcloud')]) {
							bat "${sonarRunner} end /d:\"sonar.login=%Sonarcloud%\" "
					}
				}
			}

			if (!isPr) {
				loggableStage('5. Build') {
					// back
					def config = "Release"
					def webProjFile = findFiles(glob: "**/CloudControl.Web.csproj").first().path
					bat "dotnet publish ${webProjFile} -p:VersionPrefix=${prefix} -p:VersionSuffix=${suffix} -p:AssemblyVersion=${semver} -o ${WORKSPACE}\\${buildDirectory}\\back -c ${config} -f netcoreapp3.1 -r win10-x64 /nodereuse:false --verbosity m"

					withCredentials([file(credentialsId: '86b37cd3-224e-4c64-b90d-843764ba9d30', variable: 'devops_config')]) {
					}
				}

				loggableStage('6. Archive') {
					// back
					zip archive:true, dir: "${buildDirectory}\\back\\", glob: '**/*', zipFile: "${archiveDirectory}/zips/${projectTechnicalName}.back.zip"

					// prod
					archiveArtifacts artifacts: '.cd/production.json'
				}
			}
		}
	} catch(err) {
		println err
		currentBuild.result = 'failure'
	} finally {
		loggableStage('Notify') {
			notifyEndStats()
			slackBuildEnd scmVars: scmVars
		}
	}
}
