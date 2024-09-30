@Library('jenkins-common')_
Devops = new io.bddevops.devops.Devops(this)

def targetVersion = "5.8.0"

podTemplate(cloud: 'Rancher', yaml: """
apiVersion: v1
kind: Pod
metadata:
  labels:
    jenkins/kube-default: true
    app: jenkins
    component: agent
spec:
  containers:
    - name: jnlp
      image: jenkins/inbound-agent:windowsservercore-ltsc2019
      env:
        - name: JENKINS_AGENT_WORKDIR
          value: 'C:/home/jenkins/agent/workspace'
    - name: dotnet
      image: pyxis-docker.docker-reg.bddevops.io/pyxis-build-dotnet-framework-sdk4.8:1.0.0.26
      command: ['powershell']
      args: [Import-Module '.\\MsSqlServerModule.psm1' ; Start-Server -InstanceName \"MSSQLSERVER\" -Wait ; Set-SysAdminPassword -SaPassword \"Str0ngP@ssw04d\" ; Start-Sleep 999999;]
  nodeSelector:
    kubernetes.io/os: windows
    node.kubernetes.io/windows-build: 10.0.17763
"""
)
{
  def isReleaseBranch = false
  def isMainBranch = false
  def isPullRequest = false

  node(POD_LABEL) {
    stage('Pre-Build') {
      Devops.enableSemanticVersioning()      
      def isReleaseBranchRegex = '^\\d+.\\d+$'
      isReleaseBranch = Devops.steps.env.BRANCH_NAME.matches(isReleaseBranchRegex)
      isMainBranch = Devops.steps.env.BRANCH_NAME == 'main'
      isPullRequest = !(isReleaseBranch || isMainBranch)

      if(!isReleaseBranch) {
        Devops.setToPreReleaseBranch()
      }
      else {
        Devops.setToReleaseBranch(Devops.steps.env.BRANCH_NAME)
      }
        
      Devops.preBuild(targetVersion)          
    }

    container('dotnet') {
      stage('Build') {
        def slnFiles = ["src\\dispensing-shared.sln"]
        Devops.msbuild(slnFiles)
      }
      
      stage('Unit Tests') {
        Devops.Dotnet.setDotnetTestCollect('"XPlat Code Coverage;Format=opencover"')
        Devops.dotnetTest("**/Dispensing.Tests.csproj")
      }

      stage('Pack') {
        Devops.packNuspecList()
      }
    }

    stage ('Post-Build') {
      Devops.postBuild()
    }
  }
}
