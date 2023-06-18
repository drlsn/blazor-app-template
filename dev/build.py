import os
import sys
import re
import shutil

def generate_dockerignore(dockerfile_name, image_name):

    print("Search folder in Dockerfile started")

    if dockerfile_name in os.listdir('.'):
        print(f'{dockerfile_name} exists')
    else:
        print(f'{dockerfile_name} does not exist')
        sys.exit(1)

    pattern = r'COPY\s+.*\.csproj\s*.*'

    csproj_references = []
    with open(f'{dockerfile_name}', 'r') as dockerfile:
        for line in dockerfile:
            match = re.search(pattern, line)
            if match:
                reference = match.group(0)
                csproj_references.append(reference)

    print("\ncsproj_references:")
    for element in csproj_references:
        print(element)
    
    csproj_names = []

    for line in csproj_references:
        match = re.search(r'COPY \[".*\/(.*\.csproj)"', line)
        if match:
            csproj_name = match.group(1)
            csproj_names.append(csproj_name)

    print("\ncsproj names:")
    for element in csproj_names:
        print(element)

    csproj_basenames = []

    for line in csproj_names:
        basename = os.path.splitext(line)[0]
        csproj_basenames.append(basename)

    print("\ncsproj base names:")
    for element in csproj_basenames:
        print(element)

    csproj_paths = []

    for line in csproj_references:
        match = re.search(r'COPY \["(.*\.csproj)"', line)
        if match:
            csproj_path = match.group(1)
            csproj_paths.append(csproj_path)

    print("\ncsproj paths:")
    for element in csproj_paths:
        print(element)

    for path in csproj_paths:
        shutil.copy2(path, path + '.copy')
    
    for path in csproj_paths:
        with open(path, 'r') as file:
            content = file.read()
            
            pattern = r'<(PackageReference|ProjectReference).*Include="([^"]*\\)?({})\.csproj"'.format('|'.join(map(re.escape, csproj_basenames)))
            
            matches = re.findall(pattern, content)
            for match in matches:
                basename = match[2]  # Extract the matched basename from the third group
                print(f'Matched basename: {basename}')
                new_reference = '<ProjectReference Include="{}"'.format(f'../{basename}/{basename}.csproj')
                content = re.sub(pattern, new_reference, content, flags=re.MULTILINE)
                with open(path, 'w') as file:
                    file.write(content)

            print(content)

    
    contextFolders = []

    for line in csproj_references:
        match = re.search(r'\["([^/]+)/', line)
        if match:
            folder = match.group(1)
            contextFolders.append(folder)

    print("\nContext folders:")
    for element in contextFolders:
        print(element)

    all_folders = [folder for folder in os.listdir('.') if os.path.isdir(os.path.join('.', folder))]
    exclude_folders = [folder for folder in all_folders if folder not in contextFolders]

    other_folders = [
        '**/.classpath',
        '**/.dockerignore',
        '**/.env',
        '**/.git',
        '**/.gitignore',
        '**/.project',
        '**/.settings',
        '**/.toolstarget',
        '**/.vs',
        '**/.vscode',
        '**/*.*proj.user',
        '**/*.dbmdl',
        '**/*.jfm',
        '**/azds.yaml',
        '**/bin',
        '**/charts',
        '**/docker-compose*',
        '**/Dockerfile*',
        '**/node_modules',
        '**/npm-debug.log',
        '**/obj',
        '**/secrets.dev.yaml',
        '**/values.dev.yaml',
        'LICENSE',
        'README.md'
    ]

    exclude_folders.extend(other_folders)
    
    print("\Exclude folders:")
    for element in exclude_folders:
        print(element)
    
    # Create the .dockerignore file
    if exclude_folders:
        with open('.dockerignore', 'w') as f:
            f.write('\n'.join(exclude_folders))

    # Run the docker build command
    build_command = f'docker build -t {image_name} -f {dockerfile_name} .'
    os.system(build_command)

    os.remove('.dockerignore')
     
    print('Replace .csproj files with their .copy backups')
    for path in csproj_paths:
        backup_path = path + '.copy'
        shutil.copyfile(backup_path, path)

    print('Delete the backups')
    for path in csproj_paths:
        backup_path = path + '.copy'
        os.remove(backup_path)

if __name__ == "__main__":
    # Check if the correct number of command-line arguments is provided
    if len(sys.argv) != 3:
        print("Provide arguments")
        sys.exit(1)

    dockerfile_name = sys.argv[1]
    image_name = sys.argv[2]
    generate_dockerignore(dockerfile_name, image_name)