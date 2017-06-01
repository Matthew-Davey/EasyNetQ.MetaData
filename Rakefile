require 'rubygems'
require 'bundler/setup'
require 'albacore'
require 'albacore/tasks/versionizer'

task :default => [:clean, :version, :build]

desc 'Clean up the working folder, deletes bin and obj'
build :clean do |build|
  rm_rf 'pkg'
  build.nologo
  build.sln = 'EasyNetQ.MetaData.sln'
  build.target = [ :Clean ]
  build.prop 'configuration', build_configuration
  build.logging = 'detailed'
end

desc 'Extract version information from .semver'
Albacore::Tasks::Versionizer.new :read_semver

desc 'Writes out the AssemblyVersion file'
asmver :version => [:read_semver] do |file|
  file.file_path = './AssemblyVersion.cs'
  file.attributes assembly_version: ENV['FORMAL_VERSION'],
    assembly_file_version: ENV['BUILD_VERSION'],
    assembly_informational_version: ENV['NUGET_VERSION']
end

desc 'Restores missing nuget packages'
nugets_restore :package_restore do |nuget|
    nuget.out = 'packages'
    nuget.nuget_gem_exe
end

desc 'Executes msbuild/xbuild against the project file'
build :build => [:clean, :version, :package_restore] do |build|
  build.nologo
  build.sln = 'EasyNetQ.MetaData.sln'
  build.target = [ :Build ]
  build.prop 'configuration', build_configuration
  build.logging = 'detailed'
  build.add_parameter '/consoleloggerparameters:PerformanceSummary;Summary;ShowTimestamp'
end

desc 'Writes out the nuget package for the current version'
nugets_pack :package => [:build] do |nuget|
  Dir.mkdir('pkg') unless Dir.exist?('pkg')
  nuget.configuration = build_configuration
  nuget.files = FileList['EasyNetQ.MetaData/EasyNetQ.MetaData.csproj']
  nuget.out = 'pkg'
  nuget.nuget_gem_exe
  nuget.with_metadata do |meta|
    meta.version = ENV['NUGET_VERSION']
    meta.authors = 'Matt Davey'
    meta.description = 'An extension to EasyNetQ that allows you to utilize message headers, without resorting to AdvancedBus!'
    meta.project_url = 'https://github.com/Matthew-Davey/EasyNetQ.MetaData'
    meta.tags = 'amqp rabbitmq easynetq header'
  end
end

task :publish => [:package] do
  package = "pkg/EasyNetQ.MetaData.#{ENV['NUGET_VERSION']}.nupkg"
  nuget = Albacore::Nugets::find_nuget_gem_exe
  system("#{nuget} push #{package} #{ENV['NUGET_API_KEY']} -NonInteractive -Verbosity detailed")
end

def build_configuration
  return ENV['configuration'] || 'Debug'
end
