version: 1.0
builds:
  LinuxTestBuild: # replace with the name for your build
    executableName: Game.x86_64 # the name of your build executable
    buildPath: Builds/Server # the location of the build files
buildConfigurations:
  LinuxBuildConfiguration: # replace with the name for your build configuration
    build: LinuxTestBuild # replace with the name for your build
    binaryPath: Game.x86_64 # the name of your build executable
    commandLine: -port $$port$$ -queryport $$query_port$$ -logFile $$log_dir$$/Engine.log -batchmode -nographics   # launch parameters for your server
    variables: {}
    cores: 1 # number of cores per server
    speedMhz: 750 # launch parameters for your server
    memoryMiB: 800 # launch parameters for your server
fleets:
  LinuxTestFleet: # replace with the name for your fleet
    buildConfigurations:
      - LinuxBuildConfiguration # replace with the names of your build configuration
    regions:
      Europe: # North America, Europe, Asia, South America, Australia
        minAvailable: 0 # minimum number of servers running in the region
        maxServers: 10 # maximum number of servers running in the region
