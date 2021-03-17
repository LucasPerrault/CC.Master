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
    def reportFolderName = "CC.Master-${env.BUILD_NUMBER}-${UUID.randomUUID().toString()}"

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

				setupFront(nodeJsVersion: "Node LTS v12.x.y")

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

				loggableStage('Living Doc') {
					dir("${WORKSPACE}@tmp") {
						bat "dotnet tool install --no-cache --tool-path=${WORKSPACE}\\.jenkins\\tools SpecFlow.Plus.LivingDoc.CLI"
					}

					bat """
						node back\\Tools\\livingdoc.js
						livingdoc feature-folder back\\ -t testexecution.json --title ${repoName}
						exit /b 0
					"""

					bat """
						mkdir \\\\labs2\\c\$\\d\\sites\\recette-auto\\${reportFolderName}\\
						copy LivingDoc.html \\\\labs2\\c\$\\d\\sites\\recette-auto\\${reportFolderName}\\
						echo "https://recette-auto.lucca.fr/${reportFolderName}/LivingDoc.html"
						exit /b 0
					"""
					slackWarning channel: "cc-ci-cd", message: "${env.BRANCH_NAME} (livingdoc)\n https://recette-auto.lucca.fr/${reportFolderName}/LivingDoc.html"
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
