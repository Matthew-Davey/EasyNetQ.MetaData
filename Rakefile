require "rubygems"
require "bundler/setup"
require "semver"

@build_configuration = ENV["CONFIGURATION"] || "Release"
@verbosity = ENV["VERBOSITY"] || "detailed"
@semver = SemVer.find

desc "Writes the semantic version of the project to stdout"
task :semver do
	puts(@semver.to_s)
end

desc "Clean up the working folder"
task :clean do
	sh "dotnet clean --verbosity #{@verbosity} --configuration #{@build_configuration}"
end

desc "Restore missing nuget packages"
task :package_restore do
	sh "dotnet restore --verbosity #{@verbosity}"
end

desc "Executes dotnet build in the project root folder"
task :build => [:clean, :package_restore] do
	sh "dotnet build --verbosity #{@verbosity} --configuration #{@build_configuration} --no-restore /p:Version=#{@semver.format "%M.%m.%p"}"
end

desc "Builds nuget packages for any project which is configured to generate a nuget package"
task :package => [:build] do
	Dir.mkdir("pkg") unless Dir.exist?("pkg")
	sh "dotnet pack --configuration #{@build_configuration} --no-build --no-restore --verbosity #{@verbosity} /p:Version=#{@semver.format "%M.%m.%p"} --output ../pkg"
end

desc "Publishes the nuget package(s) to the Nuget repository"
task :publish => [:package] do
	sh "dotnet nuget push ./artifacts/*.nupkg --source #{ENV["NUGET_FEED_URL"]} --api-key #{ENV["NUGET_API_KEY"]}"
end

task :default => [:build]