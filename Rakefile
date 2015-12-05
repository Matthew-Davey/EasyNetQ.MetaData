require 'rubygems'
require 'bundler/setup'
require 'albacore'
require 'albacore/tasks/versionizer'

task :default => [:clean, :version, :build]

desc 'Clean up the working folder, deletes bin and obj'
build :clean do |build|
  rm_rf 'pkg'
  build.file = 'EasyNetQ.MetaData.sln'
  build.target = [ :Clean ]
  build.prop 'configuration', build_configuration
end

desc 'Extract version information from .semver'
Albacore::Tasks::Versionizer.new :read_semver

desc 'Writes out the AssemblyVersion file'
asmver :version => [:read_semver] do |file|
  file.file_path = 'EasyNetQ.MetaData/Properties/AssemblyVersion.cs'
  file.attributes assembly_version: ENV['FORMAL_VERSION'],
    assembly_file_version: ENV['BUILD_VERSION'],
    assembly_informational_version: ENV['NUGET_VERSION']
end

desc 'Executes msbuild/xbuild against the project file'
build :build => [:clean, :version] do |build|
  build.file = 'EasyNetQ.MetaData.sln'
  build.target = [ :Build ]
  build.prop 'configuration', build_configuration
end

desc 'Writes out the nuget package for the current version'
nugets_pack :package => [:build] do |nuget|
  Dir.mkdir('pkg') unless Dir.exist?('pkg')
  nuget.configuration = build_configuration
  nuget.files = FileList['EasyNetQ.MetaData/EasyNetQ.MetaData.csproj']
  nuget.out = 'pkg'
  nuget.exe = '.nuget/NuGet.exe'
  nuget.with_metadata do |meta|
    meta.version = ENV['NUGET_VERSION']
    meta.authors = 'Matt Davey'
    meta.description = 'An extension to EasyNetQ that allows you to utilize message headers, without resorting to AdvancedBus!'
    meta.project_url = 'https://github.com/Matthew-Davey/EasyNetQ.MetaData'
    meta.tags = 'amqp rabbitmq easynetq header'
  end
  nuget.with_package do |pkg|
    docFile = "EasyNetQ.MetaData/bin/#{build_configuration}/EasyNetQ.MetaData.XML"
    pkg.add_file docFile, 'lib/net40' if File.exist?(docFile)
  end
end

task :publish => [:package] do
  package = "pkg/EasyNetQ.MetaData.#{ENV['NUGET_VERSION']}.nupkg"
  nuget_exe = '.nuget/NuGet.exe'
  system "#{nuget_exe} push #{package} #{ENV['NUGET_API_KEY']}"
end

def build_configuration
  return ENV['configuration'] || 'Debug'
end
