@Library('Lucca@v0.25.0')

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

	try {
		timeout(time: 15, unit: 'MINUTES') {


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

				computeVersion()

				installDevtools()

			}
			
				cleanBack(slnFilepath: slnFilepath)

			if(isPr || isMainBranch) {
				sonar(
					repoName: repoName,
					sonarProjectName: projectTechnicalName,
					exclusions: "**/Migrations/**",
				) {
					cleanBack(slnFilepath: slnFilepath)
					buildBack(slnFilepath: slnFilepath)
					testBack(slnFilepath: slnFilepath)
					}
			}

			if (!isPr) {
				loggableStage('5. Build') {
					// back
					def webProjFile = findFiles(glob: "**/CloudControl.Web.csproj").first().path
                    publishBack(startupProjFilepath: webProjFile, framework: "netcoreapp3.1")
					}

				archiveElements(back: true, front: false)
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
